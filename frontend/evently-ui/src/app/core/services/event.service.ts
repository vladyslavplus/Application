import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface EventDto {
  id: string;
  title: string;
  description?: string;
  startDate: string;
  endDate?: string;
  location: string;
  capacity?: number | null; 
  isPublic: boolean;
  participantCount: number;
  organizerId: string;
  isJoined?: boolean;
}

export interface EventDetailDto extends EventDto {
  participants: ParticipantDto[];
}

export interface ParticipantDto {
  userId: string;
  fullName: string;
}

export interface EventCreateDto {
  title: string;
  description?: string;
  startDate: string;
  endDate?: string;
  location: string;
  capacity?: number | null;
  isPublic: boolean;
}

@Injectable({ providedIn: 'root' })
export class EventService {
  private apiUrl = `${environment.apiUrl}/events`;

  constructor(private http: HttpClient) {}

  getPublicEvents() {
    return this.http.get<EventDto[]>(this.apiUrl);
  }

  getEvent(id: string) {
    return this.http.get<EventDto>(`${this.apiUrl}/${id}`);
  }

  getUserEvents() {
    return this.http.get<EventDto[]>(`${this.apiUrl}/me`);
  }

  getEventById(id: string) {
    return this.http.get<EventDetailDto>(`${this.apiUrl}/${id}`);
  }

  createEvent(data: EventCreateDto) {
    return this.http.post<EventDto>(this.apiUrl, data);
  }

  updateEvent(id: string, data: Partial<EventDto>) {
    return this.http.patch(`${this.apiUrl}/${id}`, data);
  }

  deleteEvent(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  joinEvent(id: string) {
    return this.http.post(`${this.apiUrl}/${id}/join`, {});
  }

  leaveEvent(id: string) {
    return this.http.post(`${this.apiUrl}/${id}/leave`, {});
  }
}