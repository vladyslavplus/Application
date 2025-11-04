import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { EventsList } from './features/events/pages/events-list/events-list';
import { EventDetails } from './features/events/pages/event-details/event-details';
import { CreateEvent } from './features/events/pages/create-event/create-event';
import { MyEvents } from './features/events/pages/my-events/my-events';

export const routes: Routes = [
  { path: '', redirectTo: '/events', pathMatch: 'full' },
  { path: 'events', component: EventsList },
  { path: 'events/create', component: CreateEvent },
  { path: 'events/:id', component: EventDetails },
  { path: 'my-events', component: MyEvents },
  { path: 'auth/login', component: Login },
  { path: 'auth/register', component: Register },
];