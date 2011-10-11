COMPILE_TARGET = "debug"
require "./utils.rb"

PRODUCT = "AuctionSniper"
CLR_VERSION = 'v4.0.30319'


task :default => [:compile, :unit_test]

task :compile  do
  buildRunner = MSBuildRunner.new(CLR_VERSION)
  buildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => '../auction-sniper.sln'
end


task :check_openfire_server  do
  require 'uri'
  require 'net/http'

  uri = URI.parse("http://localhost:9090")
  response = Net::HTTP.get_response(uri)
  unless response.code == '200' 
    fail "openfire sever my not be running"
  end
end



