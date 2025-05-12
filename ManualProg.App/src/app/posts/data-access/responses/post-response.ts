export interface IPostResponse {
  id: string;
  imageIds: string[];
  description:  string;
  commentCount: number;
  likeCount: number;
  profile: IPostResponseProfileData;
  created: Date;
}

export interface IPostResponseProfileData {
  id: string;
  name: string;
  hasImage: boolean;
}
