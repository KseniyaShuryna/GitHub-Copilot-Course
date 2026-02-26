import api from './client';
import type { TaskDto, CreateTaskRequest, UpdateTaskRequest, ApiError } from './types';

function extractApiError(error: unknown): ApiError {
  if (typeof error === 'object' && error !== null && 'response' in error) {
    const err = error as any;
    if (err.response && err.response.data && typeof err.response.data.error === 'string') {
      return err.response.data as ApiError;
    }
  }
  return { error: 'Unknown error' };
}

export async function getTasks(): Promise<TaskDto[]> {
  try {
    const res = await api.get<TaskDto[]>('/todo');
    return res.data;
  } catch (error) {
    throw extractApiError(error);
  }
}

export async function createTask(data: CreateTaskRequest): Promise<TaskDto> {
  try {
    const res = await api.post<TaskDto>('/todo', data);
    return res.data;
  } catch (error) {
    throw extractApiError(error);
  }
}

export async function updateTask(id: number, data: UpdateTaskRequest): Promise<TaskDto> {
  try {
    // For toggle, use PUT /todo/{id}/toggle, for general update, use PATCH/PUT /todo/{id} if implemented
    const res = await api.put<TaskDto>(`/todo/${id}/toggle`, data);
    return res.data;
  } catch (error) {
    throw extractApiError(error);
  }
}

export async function deleteTask(id: number): Promise<void> {
  try {
    await api.delete(`/todo/${id}`);
  } catch (error) {
    throw extractApiError(error);
  }
}
