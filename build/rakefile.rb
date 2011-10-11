COMPILE_TARGET = "debug"
require "./utils.rb"

BUILD_NUMBER = "0.9.0."
PRODUCT = "AuctionSniper"
CLR_VERSION = 'v4.0.30319'
versionNumber = ENV["CCNetLabel"].nil? ? 0 : ENV["CCNetLabel"]

task :default => [:compile, :unit_test]

task :compile  do
  buildRunner = MSBuildRunner.new(CLR_VERSION)
  buildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => '../auction-sniper.sln'
end

task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET
  runner.executeTests ['StoryTeller.Testing', 'HtmlTags.Testing']
end




