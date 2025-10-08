import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ProductosService, Producto } from '../services/productos.service';

@Component({
  selector: 'app-productos',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatTooltipModule
  ],
  templateUrl: './productos.component.html',
  styleUrls: ['./productos.component.css']
})
export class ProductosComponent implements OnInit {
  productos: Producto[] = [];
  productoForm: FormGroup;
  isEditing = false;
  editingId: number | null = null;
  displayedColumns: string[] = ['id', 'nombre', 'precio', 'stock', 'acciones'];

  constructor(
    private productosService: ProductosService,
    private fb: FormBuilder
  ) {
    this.productoForm = this.fb.group({
      nombre: ['', [Validators.required, Validators.maxLength(200)]],
      precio: [0, [Validators.required, Validators.min(0.01)]],
      stock: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadProductos();
  }

  loadProductos(): void {
    this.productosService.getProductos().subscribe({
      next: (productos) => {
        this.productos = productos;
      },
      error: (error) => {
        console.error('Error loading productos:', error);
      }
    });
  }

  onSubmit(): void {
    if (this.productoForm.valid) {
      const productoData = this.productoForm.value;
      
      if (this.isEditing && this.editingId) {
        // Update existing producto
        const producto: Producto = {
          id: this.editingId,
          ...productoData
        };
        
        this.productosService.updateProducto(this.editingId, producto).subscribe({
          next: () => {
            this.loadProductos();
            this.resetForm();
          },
          error: (error) => {
            console.error('Error updating producto:', error);
          }
        });
      } else {
        // Create new producto
        this.productosService.createProducto(productoData).subscribe({
          next: () => {
            this.loadProductos();
            this.resetForm();
          },
          error: (error) => {
            console.error('Error creating producto:', error);
          }
        });
      }
    }
  }

  editProducto(producto: Producto): void {
    this.isEditing = true;
    this.editingId = producto.id;
    this.productoForm.patchValue({
      nombre: producto.nombre,
      precio: producto.precio,
      stock: producto.stock
    });
  }

  deleteProducto(id: number): void {
    if (confirm('¿Está seguro de que desea eliminar este producto?')) {
      this.productosService.deleteProducto(id).subscribe({
        next: () => {
          this.loadProductos();
        },
        error: (error) => {
          console.error('Error deleting producto:', error);
        }
      });
    }
  }

  resetForm(): void {
    this.productoForm.reset();
    this.isEditing = false;
    this.editingId = null;
  }

  cancelEdit(): void {
    this.resetForm();
  }
}
