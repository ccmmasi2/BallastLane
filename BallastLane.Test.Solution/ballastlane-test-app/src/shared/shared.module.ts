import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MaterialModule } from './material.module';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';

@NgModule({
  declarations: [LoadingSpinnerComponent, ConfirmDialogComponent],
  imports: [CommonModule, MaterialModule],
  exports: [CommonModule, MaterialModule, LoadingSpinnerComponent, ConfirmDialogComponent],
})
export class SharedModule {}
