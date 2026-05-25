import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-category-table',
  standalone: false,
  templateUrl: './category-table.component.html',
  styleUrls: ['./category-table.component.scss'],
})
export class CategoryTableComponent {
  @Input() categories: Category[] = [];
  @Input() isLoading = false;
  @Output() edit = new EventEmitter<Category>();
  @Output() delete = new EventEmitter<Category>();

  readonly displayedColumns: string[] = ['id', 'name', 'description', 'actions'];

  onEdit(category: Category): void {
    this.edit.emit(category);
  }

  onDelete(category: Category): void {
    this.delete.emit(category);
  }
}
