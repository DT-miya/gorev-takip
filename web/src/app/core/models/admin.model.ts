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

// Backend DTO'su ile %100 uyumlu Jenerik Sayfalama Tipi
export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages?: number;
}

// Backward compatibility (eski kodlarda PagedResponse kullanılıyorsa kırılma olmaması için):
export type PagedResponse<T> = PagedResult<T>;

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