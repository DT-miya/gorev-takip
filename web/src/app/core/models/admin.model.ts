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

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    totalPages: number;
}

export interface ActivityLog {
  id: number;
  userId: number;
  userName: string;
  action: string;
  description: string;
  projectId?: number;
  taskId?: number;
  createdAt: string;
  ipAddress: string;
}