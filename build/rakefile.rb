COMPILE_TARGET = "debug"
require "./utils.rb"

PRODUCT = "AuctionSniper"
CLR_VERSION = 'v4.0.30319'
MSBUILD_DIR = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', CLR_VERSION)

NUNIT_DIR = '../lib/nunit-2.5/bin/net-2.0/'

@nunitRunner = NUnitRunner.new :compile => COMPILE_TARGET, :nunit_dir => NUNIT_DIR

task :default => [:compile, :unit_test, :integration_test, :acceptance_test]

task :compile  do
  include FileTest
  
  buildRunner = MSBuildRunner.new(MSBUILD_DIR)
  buildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => '../auction-sniper.sln'
Dir.mkdir 'output' unless exists?('output')

end

task :unit_test => [ :compile] do
   @nunitRunner.executeTests ['AuctionSniper.Unit.Tests']
end  


task :integration_test => [ :check_openfire_running] do
   @nunitRunner.executeTests ['AuctionSniper.Integration.Tests']
end  

task :acceptance_test => [ :check_openfire_running] do
   @nunitRunner.executeTests ['AuctionSniper.Acceptance.Tests']
end  

task :check_openfire_running  do
  require 'uri'
  require 'net/http'

  uri = URI.parse("http://localhost:9090")
  response = Net::HTTP.get_response(uri)
  unless response.code == '200' 
    fail "openfire sever my not be running"
  end
end



