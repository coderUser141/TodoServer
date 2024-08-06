import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from '@danielmoncada/angular-datetime-picker';
import { FormsModule } from '@angular/forms';

@NgModule({
	declarations: [
		AppComponent
	],
	imports: [
		BrowserModule, BrowserAnimationsModule, HttpClientModule, OwlDateTimeModule, OwlNativeDateTimeModule, AppRoutingModule, FormsModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
