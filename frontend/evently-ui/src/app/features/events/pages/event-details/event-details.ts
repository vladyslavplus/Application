import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { BehaviorSubject, firstValueFrom, switchMap, tap } from 'rxjs';
import { toSignal } from '@angular/core/rxjs-interop';
import { EventService, EventDetailDto, EventDto } from '../../../../core/services/event.service';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-event-details',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './event-details.html',
})
export class EventDetails {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  auth = inject(AuthService);

  isEditModalOpen = signal(false);
  Infinity = Number.POSITIVE_INFINITY;

  private refresh$ = new BehaviorSubject<void>(undefined);

  eventForm = this.fb.group({
    title: ['', Validators.required],
    description: [''],
    location: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
    capacity: [null as number | null],
  });

  event$ = this.refresh$.pipe(
    switchMap(() =>
      this.route.paramMap.pipe(
        switchMap((params) => {
          const id = params.get('id')!;
          return this.eventService.getEventById(id).pipe(
            tap((ev) => {
              this.eventForm.patchValue({
                title: ev.title,
                description: ev.description,
                location: ev.location,
                startDate: this.formatDateForInput(ev.startDate),
                endDate: this.formatDateForInput(ev.endDate ?? ev.startDate),
                capacity: ev.capacity ?? this.Infinity,
              });
            })
          );
        })
      )
    )
  );

  event = toSignal<EventDetailDto | null>(this.event$, { initialValue: null });

  toggleJoin() {
    const ev = this.event();
    if (!ev) return;

    const obs = ev.isJoined
      ? this.eventService.leaveEvent(ev.id)
      : this.eventService.joinEvent(ev.id);

    obs.pipe(tap(() => this.refresh$.next())).subscribe();
  }

  async deleteEvent(id: string) {
    if (!confirm('Are you sure you want to delete this event?')) return;

   await firstValueFrom(this.eventService.deleteEvent(id));
    this.router.navigate(['/events']);
  }

  openEditModal() {
    this.isEditModalOpen.set(true);
  }

  closeEditModal() {
    this.isEditModalOpen.set(false);
  }

  async saveChanges() {
    const ev = this.event();
    if (!ev) return;

    const raw = this.eventForm.value;
    const sanitized = Object.fromEntries(
      Object.entries(raw).map(([k, v]) => [k, v === null ? undefined : v])
    ) as Record<string, any>;

    const updated: Partial<EventDto> = {
      ...ev,
      ...sanitized,
      capacity:
        sanitized['capacity'] === this.Infinity
          ? null
          : sanitized['capacity'] && sanitized['capacity'] > 0
          ? sanitized['capacity']
          : null,
    };

    await firstValueFrom(this.eventService.updateEvent(ev.id, updated));
    this.isEditModalOpen.set(false);
    this.refresh$.next();
  }

  isOrganizer(): boolean {
    const ev = this.event();
    return !!ev && ev.organizerId === this.auth.getTokenUserId();
  }

  onCapacityInput(event: Event) {
    const input = event.target as HTMLInputElement;
    const value = Number(input.value);

    if (!input.value || value === 0) {
      this.eventForm.patchValue({ capacity: this.Infinity });
      return;
    }

    if (value < 0) {
      input.value = '1';
      this.eventForm.patchValue({ capacity: 1 });
      return;
    }

    this.eventForm.patchValue({ capacity: value });
  }

  private formatDateForInput(date: string | Date | null): string {
    if (!date) return '';
    const d = new Date(date);
    const offset = d.getTimezoneOffset();
    const local = new Date(d.getTime() - offset * 60 * 1000);
    return local.toISOString().slice(0, 16);
  }
}