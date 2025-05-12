export interface IUpdateProfileRequest {
  name: string;
  description: string | null;
  image: File;
}
