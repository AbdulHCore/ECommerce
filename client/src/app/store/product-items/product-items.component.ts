import { Component, Input } from '@angular/core';
import { IProduct } from '../../shared/models/product';

@Component({
  selector: 'app-product-items',
  templateUrl: './product-items.component.html',
  styleUrl: './product-items.component.scss'
})
export class ProductItemsComponent {
  @Input() product?:IProduct;
}
