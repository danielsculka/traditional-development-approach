import { UserRole } from "../../../shared/enums/user-role";

export interface IUpdateUserRequest {
  role: UserRole;
}
