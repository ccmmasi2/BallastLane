import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Customer } from '../../models/customer.model';

@Component({
  selector: 'app-customer-table',
  standalone: false,
  templateUrl: './customer-table.component.html',
  styleUrls: ['./customer-table.component.scss'],
})
export class CustomerTableComponent {
  @Input() customers: Customer[] = [];
  @Input() isLoading = false;
  @Output() edit = new EventEmitter<Customer>();
  @Output() delete = new EventEmitter<Customer>();

  readonly displayedColumns: string[] = [
    'id',
    'fullName',
    'documentNumber',
    'email',
    'phone',
    'actions',
  ];

  onEdit(customer: Customer): void {
    this.edit.emit(customer);
  }

  onDelete(customer: Customer): void {
    this.delete.emit(customer);
  }
}
