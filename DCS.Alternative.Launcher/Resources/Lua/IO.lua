
function serializeToFile(path, name, t)
	if t then
		local file, err = io.open(path, 'w')
		
		if file then
			local s = Serializer:new(file)
			s:serialize(name, t)
			file:close()
		else
			print(err)
		end
	end
end