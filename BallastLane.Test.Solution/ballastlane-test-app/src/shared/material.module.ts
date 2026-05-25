import { NgModule } from '@angular/core';

import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDividerModule } from '@angular/material/divider';

const MATERIAL_MODULES = [
  // Layout
  MatToolbarModule,
  MatSidenavModule,
  MatListModule,
  MatCardModule,
  MatDividerModule,

  // Data display
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,

  // Forms
  MatFormFieldModule,
  MatInputModule,
  MatSelectModule,

  // Buttons & indicators
  MatButtonModule,
  MatIconModule,
  MatProgressSpinnerModule,

  // Popups & notifications
  MatDialogModule,
  MatSnackBarModule,
  MatTooltipModule,
];

@NgModule({
  imports: MATERIAL_MODULES,
  exports: MATERIAL_MODULES,
})
export class MaterialModule {}
