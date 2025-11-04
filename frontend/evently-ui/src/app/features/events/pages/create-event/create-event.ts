import { CommonModule } from '@angular/common';
import { Component, inject, signal, computed } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { EventService, EventCreateDto } from '../../../../core/services/event.service';
import { AuthService } from '../../../../core/services/auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-create-event',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './create-event.html',
})
export class CreateEvent {
  private fb = inject(FormBuilder);
  private router = inject(Router);
  private eventService = inject(EventService);
  private auth = inject(AuthService);

  isSubmitting = signal(false);
  errorMessage = signal<string | null>(null);

  form = this.fb.nonNullable.group({
    title: ['', Validators.required],
    description: [''],
    startDate: ['', Validators.required],
    endDate: [''],
    location: ['', Validators.required],
    capacity: [null as number | null],
    isPublic: [true],
  });

  isDateValid = computed(() => {
    const start = new Date(this.form.get('startDate')?.value || '');
    return start.getTime() > Date.now();
  });

  async onSubmit() {
    if (this.form.invalid || !this.isDateValid()) {
      this.errorMessage.set('Please fill all required fields and select a valid future date.');
      return;
    }

    this.isSubmitting.set(true);
    this.errorMessage.set(null);

    try {
      const eventData = this.form.value as EventCreateDto;
      const created = await firstValueFrom(this.eventService.createEvent(eventData));
      await this.router.navigate(['/events', created.id]);
    } catch (err: any) {
      this.errorMessage.set(err?.error?.message || 'Failed to create event.');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  isLoggedIn = computed(() => this.auth.loggedIn());
}