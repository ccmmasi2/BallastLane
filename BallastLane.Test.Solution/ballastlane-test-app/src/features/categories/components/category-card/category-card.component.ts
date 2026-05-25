import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-category-card',
  standalone: false,
  templateUrl: './category-card.component.html',
  styleUrls: ['./category-card.component.scss'],
})
export class CategoryCardComponent {
  @Input() category!: Category;
  @Output() edit = new EventEmitter<Category>();
  @Output() delete = new EventEmitter<Category>();

  onEdit(): void {
    this.edit.emit(this.category);
  }

  onDelete(): void {
    this.delete.emit(this.category);
  }
}
