import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { POSConfigComponent } from './components/pos-config/pos-config.component';

const routes: Routes = [
  {
    path: '',
    component: POSConfigComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    POSConfigComponent // Import standalone component
  ]
})
export class POSConfigModule { }
