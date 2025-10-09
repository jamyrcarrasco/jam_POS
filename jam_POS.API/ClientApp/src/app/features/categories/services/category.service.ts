import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Category, CategoryFilter } from '../models/category.model';
import { PagedResult } from '../../../core/models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}categorias`;
  }

  getCategories(filter?: CategoryFilter): Observable<PagedResult<Category>> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);
      if (filter.activo !== undefined && filter.activo !== null) params = params.set('activo', filter.activo.toString());
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
      if (filter.orderDescending !== undefined && filter.orderDescending !== null) params = params.set('orderDescending', filter.orderDescending.toString());
    } else {
      // Default pagination
      params = params.set('pageNumber', '1');
      params = params.set('pageSize', '10');
    }
    
    return this.http.get<PagedResult<Category>>(this.apiUrl, { params });
  }

  getActiveCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.apiUrl}/activas`);
  }

  getCategory(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/${id}`);
  }

  createCategory(category: Partial<Category>): Observable<Category> {
    return this.http.post<Category>(this.apiUrl, category);
  }

  updateCategory(id: number, category: Partial<Category>): Observable<Category> {
    const categoryWithId = { ...category, id };
    return this.http.put<Category>(`${this.apiUrl}/${id}`, categoryWithId);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

