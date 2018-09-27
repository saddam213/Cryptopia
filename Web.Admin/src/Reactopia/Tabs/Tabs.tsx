import * as React from 'react';
import { TabModel } from './models'; 

interface props {
	tabs: TabModel[]
}

interface state {
	activeTabIndex: number
}

export class Tabs extends React.Component<props, state> {

	constructor(props, context) {
		super(props, context);

		this.state = {
			activeTabIndex: 0
		};
	}

	private handleTabClick(tabIndex) {
		this.setState({
			activeTabIndex: tabIndex
		});
	}

	render() {
		return (
			<div>
				<ul className="nav nav-tabs">
					{this.Tabs}
				</ul>
				<br />
				{this.renderActiveTabContent}
			</div>
		);
	}

	get Tabs(): JSX.Element[] {
		var self = this;
		return this.props.tabs.map(function(tab, index) {

			let { name } = tab;

			return <li className={`${(self.state.activeTabIndex == index) ? 'active' : ''}`} key={index}>
				<a href='#'
					onClick={(event) => {
						event.preventDefault();
						self.handleTabClick(index);
					}}>
					<h5>{name}</h5>
				</a>
			</li>
		})

	}

	get renderActiveTabContent(): JSX.Element[] {
		var self = this;
		return this.props.tabs.map(function (tab, i) {
			let { content } = tab;
			return (
				<div hidden={self.state.activeTabIndex != i} key={i}>
					{ content }
				</div>
			)
		});
	}

};