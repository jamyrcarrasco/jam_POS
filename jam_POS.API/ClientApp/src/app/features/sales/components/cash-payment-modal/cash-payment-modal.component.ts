import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export interface CashPaymentData {
  totalAmount: number;
  currencySymbol: string;
  currencyDecimals: number;
}

export interface CashPaymentResult {
  amountReceived: number;
  change: number;
}

@Component({
  selector: 'app-cash-payment-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatCardModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './cash-payment-modal.component.html',
  styleUrls: ['./cash-payment-modal.component.scss']
})
export class CashPaymentModalComponent implements OnInit {
  cashForm: FormGroup;
  processing = false;

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<CashPaymentModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: CashPaymentData
  ) {
    this.cashForm = this.fb.group({
      amountReceived: ['', [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    // Enfocar el input cuando se abre el modal
    setTimeout(() => {
      const input = document.querySelector('input[formControlName="amountReceived"]') as HTMLInputElement;
      if (input) {
        input.focus();
      }
    }, 100);
  }

  getChangeAmount(): number | null {
    const amountReceived = this.cashForm.get('amountReceived')?.value;
    if (amountReceived === null || amountReceived === undefined || amountReceived === '') {
      return null;
    }
    return amountReceived - this.data.totalAmount;
  }

  getAbsChangeAmount(): number {
    const changeAmount = this.getChangeAmount();
    return changeAmount !== null ? Math.abs(changeAmount) : 0;
  }

  isValidPayment(): boolean {
    const amountReceived = this.cashForm.get('amountReceived')?.value;
    console.log("amountReceived", amountReceived);
    
    return amountReceived !== null && amountReceived !== undefined && amountReceived >= this.data.totalAmount;
  }

  formatCurrency(amount: number): string {
    const symbol = this.data.currencySymbol || '$';
    const decimals = this.data.currencyDecimals || 2;
    return `${symbol}${amount.toFixed(decimals)}`;
  }

  getStepValue(): string {
    const decimals = this.data.currencyDecimals || 2;
    return `0.${'0'.repeat(decimals - 1)}1`;
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onProcessPayment(): void {
    if (!this.isValidPayment()) {
      this.snackBar.open('La cantidad recibida debe ser mayor o igual al total', 'Cerrar', { duration: 3000 });
      return;
    }

    const amountReceived = this.cashForm.get('amountReceived')?.value;
    const change = amountReceived - this.data.totalAmount;

    const result: CashPaymentResult = {
      amountReceived: amountReceived,
      change: change
    };

    this.dialogRef.close(result);
  }
}
