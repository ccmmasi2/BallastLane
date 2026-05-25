import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Product } from '../../../products/models/product.model';

@Component({
  selector: 'app-invoice-detail-row',
  standalone: false,
  templateUrl: './invoice-detail-row.component.html',
  styleUrls: ['./invoice-detail-row.component.scss'],
})
export class InvoiceDetailRowComponent {
  @Input() detailGroup!: FormGroup;
  @Input() products: Product[] = [];
  @Input() index = 0;
  @Output() remove = new EventEmitter<number>();

  get subtotal(): number {
    const qty = Number(this.detailGroup.get('quantity')?.value) || 0;
    const price = Number(this.detailGroup.get('unitPrice')?.value) || 0;
    return qty * price;
  }

  onProductChange(productId: number): void {
    const product = this.products.find(p => p.id === productId);
    if (product) {
      this.detailGroup.patchValue({ unitPrice: product.price });
    }
  }

  onRemove(): void {
    this.remove.emit(this.index);
  }
}
