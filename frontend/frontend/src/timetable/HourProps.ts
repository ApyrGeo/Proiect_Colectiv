export interface HourProps {
  id?: string | null;
  day: string;
  period: string;
  freq?: string | null;
  location: string;
  room: string;
  format: string;
  subject: string;
  teacher: string;
  specialisation?: string | null;
}
