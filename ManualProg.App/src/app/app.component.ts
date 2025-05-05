import { Component } from '@angular/core';
import { WeatherForecast } from '../../types/weather-forecast.model';
import { WeatherService } from '../../services/weather.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'ManualProg.App';
  items: WeatherForecast[] = [];

  constructor(private service: WeatherService) {
    this.service.GetWeatherForecasts().subscribe(result => {
      this.items = result;
    })
  }

}
