export interface AdminUser {
    id: number;
    fullName: string;
    email: string;
    role: string;
}

export interface AdminStats {
    userCount: number;
    projectCount: number;
    taskCount: number;
}

export interface AdminProject {
  id: number;
  name: string;
  description?: string;
  createdAt: string;
  ownerId: number;
  ownerName: string;
  ownerEmail: string;
  columnCount: number;
  taskCount: number;
  memberCount: number;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
