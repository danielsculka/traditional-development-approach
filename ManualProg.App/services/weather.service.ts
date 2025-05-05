import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { WeatherForecast } from '../types/weather-forecast.model';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {

  constructor(private http: HttpClient) { }

  GetWeatherForecasts(): Observable<WeatherForecast[]> {
    return this.http.get<WeatherForecast[]>('api/weatherforecast')
  }
}
