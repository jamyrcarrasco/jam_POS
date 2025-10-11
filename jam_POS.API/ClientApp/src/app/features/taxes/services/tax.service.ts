import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Tax } from '../models/tax.model';

@Injectable({
  providedIn: 'root'
})
export class TaxService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}impuestos`;
  }

  getTaxes(): Observable<Tax[]> {
    return this.http.get<Tax[]>(this.apiUrl);
  }

  getActiveTaxes(): Observable<Tax[]> {
    return this.http.get<Tax[]>(`${this.apiUrl}/activos`);
  }

  getTax(id: number): Observable<Tax> {
    return this.http.get<Tax>(`${this.apiUrl}/${id}`);
  }

  createTax(tax: Partial<Tax>): Observable<Tax> {
    return this.http.post<Tax>(this.apiUrl, tax);
  }

  updateTax(id: number, tax: Partial<Tax>): Observable<Tax> {
    const taxWithId = { ...tax, id };
    return this.http.put<Tax>(`${this.apiUrl}/${id}`, taxWithId);
  }

  deleteTax(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

