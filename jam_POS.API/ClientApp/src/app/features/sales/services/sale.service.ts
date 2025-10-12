import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Sale, SaleSummary, CreateSaleRequest } from '../models/sale.model';

@Injectable({
  providedIn: 'root'
})
export class SaleService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}ventas`;
  }

  getSales(page: number = 1, pageSize: number = 10): Observable<SaleSummary[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<SaleSummary[]>(this.apiUrl, { params });
  }

  getSale(id: number): Observable<Sale> {
    return this.http.get<Sale>(`${this.apiUrl}/${id}`);
  }

  createSale(sale: CreateSaleRequest): Observable<Sale> {
    return this.http.post<Sale>(this.apiUrl, sale);
  }

  cancelSale(id: number, motivo: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/cancelar`, { motivo });
  }

  getSalesByUser(usuarioId: number, page: number = 1, pageSize: number = 10): Observable<SaleSummary[]> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<SaleSummary[]>(`${this.apiUrl}/usuario/${usuarioId}`, { params });
  }

  getSalesByDate(fechaInicio: Date, fechaFin: Date, page: number = 1, pageSize: number = 10): Observable<SaleSummary[]> {
    const params = new HttpParams()
      .set('fechaInicio', fechaInicio.toISOString())
      .set('fechaFin', fechaFin.toISOString())
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<SaleSummary[]>(`${this.apiUrl}/fecha`, { params });
  }

  getTodaySales(): Observable<{ totalVentas: number; cantidadVentas: number; fecha: Date }> {
    return this.http.get<{ totalVentas: number; cantidadVentas: number; fecha: Date }>(`${this.apiUrl}/resumen/hoy`);
  }
}
