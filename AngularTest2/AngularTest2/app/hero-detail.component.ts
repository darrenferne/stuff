import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { Location } from '@angular/common';
import 'rxjs/add/operator/switchMap';

import { Hero } from './hero';
import { HeroService } from './hero.service';

@Component({
    selector: 'hero-detail',
    templateUrl: './hero-detail.component.html',
    styleUrls: [
        './hero-detail.component.css'
    ]
})
export class HeroDetailComponent
{
    private heroService: HeroService;
    private route: ActivatedRoute;
    private location: Location;
    
    @Input()
    hero: Hero;
    
    constructor(heroService: HeroService,
                route: ActivatedRoute,
                location: Location) {

        this.heroService = heroService;
        this.route = route;
        this.location = location;
    }

    ngOnInit(): void {
        this.route.params
            .switchMap((params: Params) => this.heroService.getHero(+params['id']))
            .subscribe(hero => this.hero = hero);
    }

    save(): void {
        this.heroService.updateHero(this.hero)
            .then(() => this.goBack());
    }

    goBack(): void {
        this.location.back();
    }
}