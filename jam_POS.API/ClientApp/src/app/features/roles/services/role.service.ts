import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Role, Permission, PermissionsByModule } from '../models/role.model';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl: string;
  private permissionsUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}roles`;
    this.permissionsUrl = `${environment.apiUrl}permissions`;
  }

  getRoles(includePermissions: boolean = false): Observable<Role[]> {
    const params = new HttpParams().set('includePermissions', includePermissions.toString());
    return this.http.get<Role[]>(this.apiUrl, { params });
  }

  getActiveRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(`${this.apiUrl}/active`);
  }

  getRole(id: number): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  createRole(role: Partial<Role>): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, role);
  }

  updateRole(id: number, role: Partial<Role>): Observable<Role> {
    const roleWithId = { ...role, id };
    return this.http.put<Role>(`${this.apiUrl}/${id}`, roleWithId);
  }

  deleteRole(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.permissionsUrl);
  }

  getPermissionsByModule(): Observable<PermissionsByModule> {
    return this.http.get<PermissionsByModule>(`${this.permissionsUrl}/by-module`);
  }
}

