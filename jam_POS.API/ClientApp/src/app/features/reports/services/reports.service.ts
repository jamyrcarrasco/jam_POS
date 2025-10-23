import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { SalesReport, SalesReportFilter } from '../models/sales-report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private readonly apiUrl = `${environment.apiUrl}ventas`;

  constructor(private http: HttpClient) {}

  getSalesReport(filter: SalesReportFilter): Observable<SalesReport> {
    let params = new HttpParams();

    if (filter.month != null) {
      params = params.set('month', filter.month.toString());
    }

    if (filter.year != null) {
      params = params.set('year', filter.year.toString());
    }

    if (filter.startDate) {
      params = params.set('startDate', filter.startDate);
    }

    if (filter.endDate) {
      params = params.set('endDate', filter.endDate);
    }

    if (filter.productId != null) {
      params = params.set('productId', filter.productId.toString());
    }

    if (filter.categoryId != null) {
      params = params.set('categoryId', filter.categoryId.toString());
    }

    return this.http.get<SalesReport>(`${this.apiUrl}/ventas`, { params });
  }
}
