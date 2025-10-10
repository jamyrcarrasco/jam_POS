import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { User, UserFilter, CreateUserDto, UpdateUserDto } from '../models/user.model';
import { PagedResult } from '../../../core/models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}users`;
  }

  getUsers(filter?: UserFilter): Observable<PagedResult<User>> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);
      if (filter.roleId !== undefined && filter.roleId !== null) params = params.set('roleId', filter.roleId.toString());
      if (filter.isActive !== undefined && filter.isActive !== null) params = params.set('isActive', filter.isActive.toString());
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
      if (filter.orderDescending !== undefined && filter.orderDescending !== null) params = params.set('orderDescending', filter.orderDescending.toString());
    } else {
      params = params.set('pageNumber', '1');
      params = params.set('pageSize', '10');
    }
    
    return this.http.get<PagedResult<User>>(this.apiUrl, { params });
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(user: CreateUserDto): Observable<User> {
    return this.http.post<User>(this.apiUrl, user);
  }

  updateUser(id: number, user: UpdateUserDto): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, user);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

