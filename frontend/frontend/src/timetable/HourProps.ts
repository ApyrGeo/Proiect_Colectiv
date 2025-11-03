export interface HourProps {
  id?: string | null;
  day: string;
  period: string;
  freq: string;
  location: string;
  room: string;
  format: string;
  category: string;
  subject: string;
  teacher: string;
  specialisation?: string | null;
}
