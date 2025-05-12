export interface ICreateCommentRequest {
  postId: string;
  content: string;
  replyToCommentId: string | null;
}
