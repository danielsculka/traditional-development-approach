export interface ICreatePostRequest {
  isPublic: boolean;
  price: number | null;
  description: string;
  images: File[];
}
