import {
    Component,
    OnInit
} from '@angular/core';
import { HeroService } from './hero.service';
import { Hero } from './hero';

@Component({
    selector: 'my-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: [
        './dashboard.component.css'
    ]
})
export class DashboardComponent implements OnInit {

    private _heroService: HeroService;

    heroes: Hero[];

    constructor(heroService: HeroService) {
        this._heroService = heroService;
    }

    ngOnInit(): void {
        this.getHeroes();
    }

    getHeroes(): void {
        if (this._heroService) {
            this._heroService.getHeroes().then(heroes => this.heroes = heroes.slice(1, 5));
        }
    }
}
