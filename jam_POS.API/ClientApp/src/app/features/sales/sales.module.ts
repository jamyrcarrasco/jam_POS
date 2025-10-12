import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SalesPOSComponent } from './components/sales-pos/sales-pos.component';

const routes: Routes = [
  {
    path: '',
    component: SalesPOSComponent
  },
  {
    path: 'pos',
    component: SalesPOSComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    SalesPOSComponent // Import standalone component
  ]
})
export class SalesModule { }
