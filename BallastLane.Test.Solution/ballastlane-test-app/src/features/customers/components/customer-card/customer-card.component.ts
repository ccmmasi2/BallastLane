import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Customer } from '../../models/customer.model';

@Component({
  selector: 'app-customer-card',
  standalone: false,
  templateUrl: './customer-card.component.html',
  styleUrls: ['./customer-card.component.scss'],
})
export class CustomerCardComponent {
  @Input() customer!: Customer;
  @Output() edit = new EventEmitter<Customer>();
  @Output() delete = new EventEmitter<Customer>();

  onEdit(): void {
    this.edit.emit(this.customer);
  }

  onDelete(): void {
    this.delete.emit(this.customer);
  }
}
