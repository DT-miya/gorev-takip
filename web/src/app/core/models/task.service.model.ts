export interface TaskItem {
  id: number;
  columnId: number;
  title: string;
  description?: string;
  priority: string;
  order: number;
  assigneeId?: number;
  assigneeName?: string;
}

export interface BoardColumn {
  id: number;
  title: string;
  order: number;
  tasks: TaskItem[];
}

export interface BoardFullResponse {
  projectId: number;
  projectName: string;
  columns: BoardColumn[];
}

export interface CreateTaskRequest {
  columnId: number;
  title: string;
  description?: string;
  priority?: string;
}

export interface MoveTaskRequest {
  targetColumnId: number;
  newOrder: number;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  priority: string; // 'Low' | 'Medium' | 'High'
  assigneeId?: number | null;
  dueDate?: string | null;
}

