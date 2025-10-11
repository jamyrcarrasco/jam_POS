import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { POSConfig } from '../models/pos-config.model';

@Injectable({
  providedIn: 'root'
})
export class POSConfigService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}configuracionpos`;
  }

  getConfig(): Observable<POSConfig> {
    return this.http.get<POSConfig>(this.apiUrl);
  }

  createConfig(config: Partial<POSConfig>): Observable<POSConfig> {
    return this.http.post<POSConfig>(this.apiUrl, config);
  }

  updateConfig(id: number, config: Partial<POSConfig>): Observable<POSConfig> {
    const configWithId = { ...config, id };
    return this.http.put<POSConfig>(`${this.apiUrl}/${id}`, configWithId);
  }

  deleteConfig(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getSiguienteNumeroRecibo(): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/siguiente-recibo`);
  }

  getSiguienteNumeroFactura(): Observable<string> {
    return this.http.get<string>(`${this.apiUrl}/siguiente-factura`);
  }
}
