import { CommonModule } from '@angular/common';
import { Component, inject, signal, computed, ViewChild, AfterViewInit, HostListener } from '@angular/core';
import { Router } from '@angular/router';
import { EventService, EventDto } from '../../../../core/services/event.service';
import { AuthService } from '../../../../core/services/auth.service';
import { FullCalendarModule, FullCalendarComponent } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { CalendarOptions } from '@fullcalendar/core';

interface ColoredEventDto extends EventDto {
  color: string;
}

@Component({
  selector: 'app-my-events',
  imports: [CommonModule, FullCalendarModule],
  templateUrl: './my-events.html',
  styleUrls: ['./my-events.css'],
})
export class MyEvents implements AfterViewInit {
  private eventService = inject(EventService);
  private auth = inject(AuthService);
  private router = inject(Router);

  @ViewChild(FullCalendarComponent) calendarComponent?: FullCalendarComponent;

  calendarView = signal<'dayGridMonth' | 'dayGridWeek'>('dayGridMonth');
  calendarTitle = signal('');
  isMobile = signal(false);

  private events$ = this.eventService.getUserEvents().pipe(
    map((events) =>
      events.map((e): ColoredEventDto => ({
        ...e,
        color:
          e.organizerId === this.auth.getTokenUserId()
            ? '#3b82f6'
            : '#22c55e',
      }))
    )
  );

  events = toSignal(this.events$, { initialValue: [] as ColoredEventDto[] });

  @HostListener('window:resize')
  onResize() {
    this.updateMobileState();
  }

  private updateMobileState() {
    const wasMobile = this.isMobile();
    const nowMobile = window.innerWidth < 640;
    
    if (wasMobile !== nowMobile) {
      this.isMobile.set(nowMobile);
      if (this.calendarComponent) {
        const currentView = this.calendarView();
        this.calendarComponent.getApi().changeView(
          nowMobile && currentView === 'dayGridWeek' ? 'dayGrid3Day' : currentView
        );
      }
    }
  }

  calendarOptions = computed<CalendarOptions>(() => {
    const currentView = this.calendarView();
    const mobile = this.isMobile();
    
    const actualView = mobile && currentView === 'dayGridWeek' ? 'dayGrid3Day' : currentView;
    
    const calendarHeight = mobile 
      ? (currentView === 'dayGridWeek' ? 450 : 500)
      : undefined;
    
    const aspectRatio = !mobile
      ? (currentView === 'dayGridWeek' ? 7 : 1.35)
      : undefined;

    return {
      plugins: [dayGridPlugin, interactionPlugin],
      initialView: 'dayGridMonth',
      headerToolbar: false,
      eventClick: (info) => this.router.navigate(['/events', info.event.id]),
      datesSet: (arg) => {
        this.calendarTitle.set(arg.view.title);
      },
      events: this.events().map((e) => ({
        id: e.id,
        title: e.title,
        start: e.startDate,
        end: e.endDate ?? e.startDate,
        backgroundColor: e.color + '22',
        borderColor: e.color + '55',
        textColor: '#1e293b',
        display: 'block',
      })),
      height: calendarHeight,
      aspectRatio: aspectRatio,
      views: {
        dayGrid3Day: {
          type: 'dayGrid',
          duration: { days: 3 },
          dayHeaderFormat: { weekday: 'short', day: 'numeric', month: 'numeric' },
        },
        dayGridWeek: {
          dayHeaderFormat: mobile 
            ? { weekday: 'narrow', day: 'numeric' }
            : { weekday: 'short', day: '2-digit', month: '2-digit' },
        },
        dayGridMonth: {
          dayHeaderFormat: mobile 
            ? { weekday: 'narrow' }
            : { weekday: 'short' },
        },
      },
      eventContent: (arg) => {
        const start = arg.event.start
          ? arg.event.start.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
          : '';
        const end = arg.event.end
          ? arg.event.end.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
          : '';
        const range = start && end ? `${start} – ${end}` : start;
        
        const fontSize = mobile ? '0.7rem' : '0.75rem';
        const titleSize = mobile ? '0.75rem' : '0.8rem';
        
        return {
          html: `
            <div style="display:flex;flex-direction:column;align-items:flex-start;padding:2px;overflow:hidden;width:100%;">
              <span style="font-size:${fontSize};color:#475569;white-space:nowrap;overflow:hidden;text-overflow:ellipsis;width:100%;">${range}</span>
              <span style="font-size:${titleSize};font-weight:600;line-height:1rem;color:#1e293b;overflow:hidden;text-overflow:ellipsis;display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;width:100%;">${arg.event.title}</span>
            </div>
          `,
        };
      },
    };
  });

  ngAfterViewInit() {
    this.updateMobileState();
    setTimeout(() => {
      if (this.calendarComponent) {
        this.calendarTitle.set(this.calendarComponent.getApi().view.title);
      }
    }, 0);
  }

  next() {
    this.calendarComponent?.getApi().next();
  }

  prev() {
    this.calendarComponent?.getApi().prev();
  }

  toggleView(view: 'dayGridMonth' | 'dayGridWeek') {
    this.calendarView.set(view);
    const mobile = this.isMobile();
    const actualView = mobile && view === 'dayGridWeek' ? 'dayGrid3Day' : view;
    this.calendarComponent?.getApi().changeView(actualView);
  }
}