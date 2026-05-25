import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Product } from '../../../products/models/product.model';

@Component({
  selector: 'app-invoice-products-selector',
  standalone: false,
  templateUrl: './invoice-products-selector.component.html',
  styleUrls: ['./invoice-products-selector.component.scss'],
})
export class InvoiceProductsSelectorComponent {
  @Input() products: Product[] = [];
  @Output() addProduct = new EventEmitter<Product>();

  readonly productCtrl = new FormControl<number | null>(null);

  onAdd(): void {
    const productId = this.productCtrl.value;
    if (productId === null) return;

    const product = this.products.find(p => p.id === productId);
    if (product) {
      this.addProduct.emit(product);
      this.productCtrl.reset();
    }
  }
}
