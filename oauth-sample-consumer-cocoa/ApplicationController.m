//
//  ApplicationController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "ApplicationController.h"


@implementation ApplicationController

-(IBAction) showPreferenceController:(id)sender
{
	if(!preferenceController){
		preferenceController = [[PreferenceController alloc]init];		
	}
	[preferenceController showWindow:self];
}
@end
