export interface ICommentResponse {
  id: string;
  content: string;
  repliesCount: number;
  likeCount: number;
  hasLike: boolean;
  profile: ICommentResponseProfileData;
  created: Date;
}

export interface ICommentResponseProfileData {
  id: string;
  name: string;
  hasImage: boolean;
}
