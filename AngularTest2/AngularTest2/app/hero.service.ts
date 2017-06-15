﻿import { Injectable } from '@angular/core';
import { Headers, Http } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { Hero } from './hero';

@Injectable()
export class HeroService
{
    private heroesUrl = 'api/heroes';  // URL to web api
    private headers = new Headers({ 'Content-Type': 'application/json' });

    constructor(private http: Http) { }

    getHeroes(): Promise<Hero[]> {
        return this.http.get(this.heroesUrl)
            .toPromise()
            .then(response => response.json().data as Hero[])
            .catch(this.handleError);
    }

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error.message || error);
    }
        
    getHero(id: number): Promise<Hero> {
        var heroUrl = `${this.heroesUrl}/${id}`;
        return this.http.get(heroUrl)
            .toPromise()
            .then(response => response.json().data as Hero)
            .catch(this.handleError);
    };

    addHero(name: string): Promise<Hero> {
        return this.http
            .post(this.heroesUrl, JSON.stringify({ name: name }), { headers: this.headers })
            .toPromise()
            .then(res => res.json().data as Hero)
            .catch(this.handleError);
    }

    updateHero(hero: Hero) : Promise<Hero> {
        var heroUrl = `${this.heroesUrl}/${hero.id}`;
        return this.http
            .put(heroUrl, JSON.stringify(hero), { headers: this.headers })
            .toPromise()
            .then(() => hero)
            .catch(this.handleError);
    }

    deleteHero(id: number) : Promise<void> {
        var heroUrl = `${this.heroesUrl}/${id}`;
        return this.http
            .delete(heroUrl, { headers: this.headers })
            .toPromise()
            .then(() => null)
            .catch(this.handleError);
    }

    getHeroesSlowly(): Promise<Hero[]> {
        return new Promise(resolve => {
            // Simulate server latency with 2 second delay
            setTimeout(() => resolve(this.getHeroes()), 2000);
        });
    };
}