import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Customer, CustomerFilter, CreateCustomerDto, UpdateCustomerDto } from '../models/customer.model';
import { PagedResult } from '../../../core/models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}clientes`;
  }

  getCustomers(filter?: CustomerFilter): Observable<PagedResult<Customer>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);
      if (filter.activo !== undefined && filter.activo !== null) params = params.set('activo', filter.activo.toString());
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
      if (filter.orderDescending !== undefined && filter.orderDescending !== null) {
        params = params.set('orderDescending', filter.orderDescending.toString());
      }
    } else {
      params = params.set('pageNumber', '1');
      params = params.set('pageSize', '10');
    }

    return this.http.get<PagedResult<Customer>>(this.apiUrl, { params });
  }

  getCustomer(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  createCustomer(customer: CreateCustomerDto): Observable<Customer> {
    return this.http.post<Customer>(this.apiUrl, customer);
  }

  updateCustomer(id: number, customer: UpdateCustomerDto): Observable<Customer> {
    return this.http.put<Customer>(`${this.apiUrl}/${id}`, customer);
  }

  deleteCustomer(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getActiveCustomers(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.apiUrl}/activos`);
  }
}
