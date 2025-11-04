import { CommonModule } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, startWith, combineLatest, map, switchMap, firstValueFrom } from 'rxjs';
import { toSignal, toObservable } from '@angular/core/rxjs-interop';
import { EventService, EventDto } from '../../../../core/services/event.service';
import { AuthService } from '../../../../core/services/auth.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-events-list',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './events-list.html'
})
export class EventsList {
  private eventService = inject(EventService);
  auth = inject(AuthService);

  searchControl = new FormControl<string>('', { nonNullable: true });

  private refresh = signal(0);
  refreshEvents = () => this.refresh.update(v => v + 1);

  private refresh$ = toObservable(this.refresh);

  events$ = this.refresh$.pipe(
    switchMap(() => this.eventService.getPublicEvents())
  );

  filteredEvents$ = combineLatest([
    this.events$,
    this.searchControl.valueChanges.pipe(
      startWith(''),
      debounceTime(200),
      distinctUntilChanged()
    ),
  ]).pipe(
    map(([events, term]) => {
      const lower = (term ?? '').toLowerCase();
      return events.filter((e: EventDto) =>
        e.title.toLowerCase().includes(lower) ||
        (e.description ?? '').toLowerCase().includes(lower) ||
        e.location.toLowerCase().includes(lower)
      );
    })
  );

  filteredEvents = toSignal(this.filteredEvents$, { initialValue: [] as EventDto[] });

  async toggleJoin(event: EventDto) {
    const action$ = event.isJoined
      ? this.eventService.leaveEvent(event.id)
      : this.eventService.joinEvent(event.id);

    await firstValueFrom(action$);
    this.refreshEvents();
  }
}