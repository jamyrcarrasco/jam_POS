import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SalesReportComponent } from './components/sales-report/sales-report.component';

const routes: Routes = [
  {
    path: '',
    component: SalesReportComponent
  },
  {
    path: 'ventas',
    component: SalesReportComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    SalesReportComponent
  ]
})
export class ReportsModule {}
