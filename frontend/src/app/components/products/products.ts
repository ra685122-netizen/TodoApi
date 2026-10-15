import { Component } from '@angular/core';
import { IProduct } from '../../models/iproduct';
import { CommonModule } from '@angular/common';
import { count } from 'rxjs';
@Component({
  imports: [CommonModule],

  selector: 'app-products',
  styleUrl: './products.css',
  templateUrl: './products.html',
})
export class Products {
  Products: IProduct[];
totalOrderPrice:number=0
  constructor() {
    this.Products = [
      {
        id: 100,
        name: 'Dell laptop',
        price: 500000,
        quantity: 3,
        catId: 1,
        imgUrl: 'https://via.placeholder.com/360x360.png/CCCCCC?text=animals+dogs+veniam',
      },
      {
        id: 200,
        name: 'HP laptop',
        price: 500000,
        quantity: 0,
        catId: 1,
        imgUrl: 'https://via.placeholder.com/360x360.png/00bbcc?text=animals+cats+vero',
      },
      {
        id: 300,
        name: 'Iphone',
        price: 3874209,
        quantity: 2,
        catId: 2,
        imgUrl: 'https://via.placeholder.com/360x360.png/CCCCCC?text=animals+dogs+veniam',
      },
      {
        id: 400,
        name: 'oppo',
        price: 6000,
        quantity: 1,
        catId: 2,
        imgUrl: 'https://via.placeholder.com/360x360.png/CCCCCC?text=animals+dogs+veniam',
      },
      {
        id: 500,
        name: 'sammsung tablet',
        price: 20000,
        quantity: 0,
        catId: 3,
        imgUrl: 'https://via.placeholder.com/360x360.png/00bbcc?text=animals+cats+vero',
      },
      {
        id: 600,
        name: 'lenovo tablet',
        price: 10000,
        quantity: 4,
        catId: 3,
        imgUrl: 'https://via.placeholder.com/360x360.png/CCCCCC?text=animals+dogs+veniam',
      },
    ];
  
  }
  Buy(count:String,price:number){
      this.totalOrderPrice=+count*price
    }
}