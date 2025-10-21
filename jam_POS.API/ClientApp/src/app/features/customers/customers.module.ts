import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomerListComponent } from './components/customer-list/customer-list.component';

const routes: Routes = [
  {
    path: '',
    component: CustomerListComponent
  }
];

@NgModule({
  imports: [
    CustomerListComponent,
    RouterModule.forChild(routes)
  ]
})
export class CustomersModule { }
