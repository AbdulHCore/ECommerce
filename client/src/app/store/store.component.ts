import { Component, OnInit } from '@angular/core';
import { StoreService } from './store.service';
import { IProduct } from '../shared/models/product';
import { IType } from '../shared/models/type';
import { IBrand } from '../shared/models/brand';
import { StoreParams } from '../shared/models/storeparams';
import { HttpParams } from '@angular/common/http';

@Component({
  selector: 'app-store',
  templateUrl: './store.component.html',
  styleUrl: './store.component.scss'
})
export class StoreComponent implements OnInit {
  products: IProduct[] = [];
  types: IType[] = [];
  brands: IBrand[] = [];
  storeParams =  new StoreParams();
  constructor(private storeService: StoreService){

  }
  ngOnInit(): void {
      this.getProducts();
      this.getBrands();    
      this.getTypes();
    };


    getProducts(){      
      this.storeService.getProducts(this.storeParams).subscribe({
        next: response => this.products = response.data,
        error: error => console.log(error)      
      });
    }
    getBrands(){
      this.storeService.getBrands().subscribe({
        next: response => this.brands = response,
        error: error => console.log(error)
      });
    }
    getTypes(){
      this.storeService.getBrands().subscribe({
        next: response => this.types = response,
        error: error => console.log(error)
      })
    }

    onBrandSelected(brandId: string){
      this.storeParams.brandId = brandId;
      this.getBrands();
    }

    onTypeSelected(typeId: string){
      this.storeParams.typeId = typeId;
      this.getTypes();
    }

  }
