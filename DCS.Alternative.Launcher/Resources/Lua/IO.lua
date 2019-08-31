function serializeToFile(path, name, t, mode)
	if t then
		local file, err = io.open(path, mode or 'w')
		
		if file then
			local s = Serializer:new(file)
			s:serialize(name, t)
			file:close()
		else
			print(err)
		end
	end
end