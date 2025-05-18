import { IPagedRequest } from "../../../shared/models/paged-request";

export interface IPostsRequest extends IPagedRequest {
  profileId?: string | null;
}
