import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Product, ProductFilter } from '../models/product.model';
import { ProductImportResult } from '../models/product-import-result.model';
import { PagedResult } from '../../../core/models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl: string;

  constructor(private http: HttpClient) {
    this.apiUrl = `${environment.apiUrl}productos`;
  }

  getProducts(filter?: ProductFilter): Observable<PagedResult<Product>> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.searchTerm) params = params.set('searchTerm', filter.searchTerm);
      if (filter.categoria) params = params.set('categoria', filter.categoria);
      if (filter.precioMin !== undefined && filter.precioMin !== null) params = params.set('precioMin', filter.precioMin.toString());
      if (filter.precioMax !== undefined && filter.precioMax !== null) params = params.set('precioMax', filter.precioMax.toString());
      if (filter.stockBajo !== undefined && filter.stockBajo !== null) params = params.set('stockBajo', filter.stockBajo.toString());
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
    
    return this.http.get<PagedResult<Product>>(this.apiUrl, { params });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: Partial<Product>): Observable<Product> {
    return this.http.post<Product>(this.apiUrl, product);
  }

  updateProduct(id: number, product: Partial<Product>): Observable<Product> {
    const productWithId = { ...product, id };
    return this.http.put<Product>(`${this.apiUrl}/${id}`, productWithId);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getCategorias(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/categorias`);
  }

  downloadTemplate(): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/plantilla`, { responseType: 'blob' });
  }

  importProducts(file: File): Observable<ProductImportResult> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ProductImportResult>(`${this.apiUrl}/importar`, formData);
  }
}
